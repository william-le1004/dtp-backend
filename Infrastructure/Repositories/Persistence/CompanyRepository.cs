using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Extensions;
using Application.Features.Wallet.Events;
using Domain.Constants;
using Domain.Entities;
using Domain.Extensions;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Persistence;

public class CompanyRepository(DtpDbContext dbContext, UserManager<User> userManager,
    IPublisher publisher) : ICompanyRepository
{
    public async Task<Company> GrantCompanyAsync(Guid id)
    {
        var existCompany = await GetCompanyAsync(id, false);
        if (existCompany is null)
            throw new InvalidOperationException("Company not found.");

        existCompany.AcceptLicense();

        await CreateUserForCompanyAsync(existCompany);
        await dbContext.SaveChangesAsync();
        return existCompany;
    }

    private async Task CreateUserForCompanyAsync(Company company)
    {
        var userName = company.Email.RemoveSubstring(ApplicationConst.GmailDomain).Trim();

        if (string.IsNullOrWhiteSpace(userName))
            throw new InvalidOperationException("Derived username from company email is invalid.");

        var user = new User(
            userName: userName,
            email: company.Email,
            name: company.Name,
            address: company.Address,
            phoneNumber: company.Phone
        );

        var defaultPassword = $"{userName}{ApplicationConst.DefaultPassword}";

        var creationResult = await userManager.CreateAsync(user, defaultPassword);
        if (!creationResult.Succeeded)
        {
            var errorDetails = string.Join("; ", creationResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user account: {errorDetails}");
        }

        await userManager.AddToRoleAsync(user, ApplicationRole.OPERATOR);
        user.AssignCompany(company.Id);
    }

    public async Task<IEnumerable<Company>> GetCompaniesAsync() =>
        await dbContext.Companies
            .OrderBy(c => c.Name)
            .Include(x => x.Staffs)
            .Include(x => x.Tours)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Company?> GetCompanyAsync(Guid id, bool noTracking) =>
        await dbContext.Companies
            .Where(c => c.Id == id)
            .Include(c => c.Staffs)
            .Include(c => c.Tours)
            .ApplyNoTracking(noTracking)
            .FirstOrDefaultAsync();

    public async Task<bool> UpsertCompanyAsync(Company company)
    {
        if (company.Id == Guid.Empty)
        {
            return await CreateCompanyAsync(company);
        }

        return await UpdateCompanyAsync(company);
    }

    public async Task<bool> ExistsByNameAsync(string name) =>
        !await dbContext.Companies.AnyAsync(c => c.Name == name);

    public async Task<bool> ExistsByEmailAsync(string gmail) =>
        !await dbContext.Companies.AnyAsync(c => c.Email == gmail);

    public async Task<string> GetNameByIdAsync(Guid id) =>
        await dbContext.Companies
            .Where(c => c.Id == id)
            .Select(c => c.Name).FirstOrDefaultAsync() ?? string.Empty;

    private async Task<bool> CreateCompanyAsync(Company company)
    {
        await dbContext.Companies.AddAsync(company);
        return await SaveChangesAsync();
    }

    private async Task<bool> UpdateCompanyAsync(Company company)
    {
        var existingCompany = await GetCompanyAsync(company.Id, false)
                              ?? throw new KeyNotFoundException($"Company with ID {company.Id} not found.");

        if (company.IsDeleted)
        {
            var tours = await dbContext.Tours.Where(t => t.CompanyId == company.Id).ToListAsync();
            foreach (var tour in tours)
            {
                tour.IsDeleted = true;
            }

            var tourId = tours.Select(t => t.Id).ToList();

            var tourSchedules =
                await dbContext.TourSchedules.Include(t => t.Tour)
                    .Include(x => x.TourBookings)
                    .ThenInclude(x => x.Tickets)
                    .ThenInclude(x => x.TicketType)
                    .AsSingleQuery()
                    .Where(t => tourId.Contains(t.TourId))
                    .ToListAsync();

            foreach (var tourSchedule in tourSchedules)
            {
                tourSchedule.IsDeleted = true;
            }

            var tourScheduleId = tourSchedules.Select(t => t.Id).ToList();
            var tourBookings = await dbContext.TourBookings.Where(t => tourScheduleId.Contains(t.TourScheduleId))
                .ToListAsync();
            foreach (var tourBooking in tourBookings)
            {
                tourBooking.Cancel();
                var payment = await dbContext.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == tourBooking.Id);

                if (payment != null)
                {
                    await publisher.Publish(
                        new PaymentRefunded(payment.NetCost, tourBooking.UserId, tourBooking.Code));

                    payment.Refund();
                }
            }
        }

        dbContext.Entry(existingCompany).CurrentValues.SetValues(company);
        return await SaveChangesAsync();
    }

    private async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}