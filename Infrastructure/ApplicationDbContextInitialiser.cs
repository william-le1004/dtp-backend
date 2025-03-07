using Domain.DataModel;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Common.Constants;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        // await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    DtpDbContext context,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        // var administratorRole = new IdentityRole(Roles.Administrator);
        //
        // if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        // {
        //     await roleManager.CreateAsync(administratorRole);
        // }

        // Default users
        // var administrator = new User { UserName = "khoalol", Email = "administrator@localhost" };
        //
        // if (userManager.Users.All(u => u.UserName != administrator.UserName))
        // {
        //     administrator.IsActive = true;
        //     await userManager.CreateAsync(administrator, "Abc123!");
        //     if (!string.IsNullOrWhiteSpace(ApplicationRole.OPERATOR))
        //     {
        //         await userManager.AddToRolesAsync(administrator, [ApplicationRole.OPERATOR]);
        //     }
        // }

        var decription =
            $"Nếu bạn là người đam mê thể thao và muốn khám phá thiên nhiên thành phố biển Quy Nhơn, nhất định đừng bỏ qua trải nghiệm chèo SUP trên Hòn Khô hay tại Cồn Chim - hai trong số địa điểm thiên nhiên tuyệt đẹp tại Quy Nhơn.\n\nĐược mệnh danh là Maldives của Việt Nam, Hòn Khô (hay còn được gọi là Cù Lao Hòn Khô) chắc chắn là một điểm đến tuyệt đẹp mà bạn không thể bỏ lỡ khi du lịch đến Quy Nhơn, Bình Định.\n\nNằm trong quần thể 32 hòn đảo xinh đẹp gần bờ của tỉnh Bình Định và cách thành phố Quy Nhơn 19km, Hòn Khô có vẻ không \"khô khan\" như cái tên của nó. Đây là một địa điểm khá mới và thu hút khách du lịch với vẻ đẹp hoang sơ, yên bình.\n\nTrong hành trình khám phá Hòn Khô bằng SUP của Klook, bạn sẽ có cơ hội khám phá một hòn đảo xinh đẹp, nước trong xanh, cát trắng xóa trên những chiếc ván chèo rất chill và mới lạ vô cùng. Hành trình bắt đầu với cung đường biển thơ mộng, vượt Cầu Thị Nại - chiếc cầu vượt biển dài nhất Miền Trung, là niềm tự hào của người dân Bình Định và đi cano xé gió đến Hòn Khô.\n\nTiếp đến, bạn sẽ vùng vẫy trong làn nước mát lạnh và khám phá thế giới động vật dưới nước đầy sắc màu trong trải nghiệm lặn ngắm san hô. Chèo SUP quanh khu vực Hòn Khô và chụp ảnh với những chiếc ván SUP là hoạt động thú vị tiếp theo trong tour, đừng quên mang theo phụ kiện để có được những bức ảnh \"chất như nước cất\" nha #teamKlook!\n\nBên cạnh đó, tour chèo SUP khám phá Cồn Chim cũng thu hút không kém với hành trình khám phá khu bảo tồn chim, cò và các loài động, thực vật biển, du khách sẽ ngắm những đàn chim về tổ sau một ngày tìm thức ăn, và đặc biệt là ngắm nhìn hoàng hôn thơ mộng ngay trên chiếc SUP\n\nBữa ăn hải sản bao gồm trong tour được người dân địa phương đánh bắt và chế biến ngay trong ngày! Vì đây là tour riêng nên đảm bảo tuyệt đối sự riêng tư cũng như chất lượng tour, nên cả nhà đừng chần chừ mà hãy đặt tour ngay trên Klook nhé!";

        // Default data
        // Seed, if necessary

        var destination1Id = Guid.NewGuid();
        var destination2Id = Guid.NewGuid();
        var destination3Id = Guid.NewGuid();
        var destination4Id = Guid.NewGuid();
        var company1Id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();
        context.Categories.AddRange(
            new Category
            {
                Id = categoryId,
                Name = "Tour đi thuyền",
            },
            new Category
            {
                Id = categoryId2,
                Name = "Tour ẩm thực",
            }
        );

        context.Companies.AddRange(
            new Company
            {
                Id = company1Id,
                Name = "KhoaBug Inc.",
                Email = "contact@techinnovators.com",
                Phone = "123-456-7890",
                TaxCode = "ABC123456",
                Licensed = true,
                CreatedBy = "admin",
            }
        );

        context.Destinations.AddRange(
            new Destination
            {
                Id = destination1Id, Name = "Hòn Khô", CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
                IsDeleted = false , Latitude = "concec", Longitude = "cailol"
            }
            // ,
            // new Destination
            // {
            //     Id = destination2Id, Name = "Tháp Bánh Ít (tháp Bạc)", CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
            //     IsDeleted = false
            // },
            // new Destination
            // {
            //     Id = destination3Id, Name = "Tháp Dương Long (Tháp Ngà)", CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
            //     IsDeleted = false
            // }
            // ,
            // new Destination
            // {
            //     Id = destination4Id, Name = "Eo Gió", CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
            //     IsDeleted = false
            // }
        );

        // Create Tours next
        var tour1Id = Guid.NewGuid();

        context.Tours.AddRange(
            new Tour
            {
                Id = tour1Id,
                Title = "Tour Riêng Chèo SUP Nửa Ngày Khám Phá Quy Nhơn tại Cù Lao Hòn Khô hoặc Cồn Chim",
                CompanyId = company1Id,
                CategoryId = categoryId,
                Description = decription, CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
                IsDeleted = false
            }
        );

        var ticket1Id = Guid.NewGuid();
        var ticket2Id = Guid.NewGuid();
        var ticket3Id = Guid.NewGuid();
        var ticket4Id = Guid.NewGuid();

        // Create TicketTypes and link to Tours
        context.TicketTypes.AddRange(
            new TicketType
            {
                Id = ticket1Id, DefaultNetCost = 70.00m, MinimumPurchaseQuantity = 1,
                TicketKind = TicketKind.Adult, TourId = tour1Id
            },
            new TicketType
            {
                Id = ticket2Id, DefaultNetCost = 50.00m, MinimumPurchaseQuantity = 1,
                TicketKind = TicketKind.Child, TourId = tour1Id
            },
            new TicketType
            {
                Id = ticket3Id, DefaultNetCost = 150.00m, MinimumPurchaseQuantity = 0,
                TicketKind = TicketKind.PerGroupOfThree, TourId = tour1Id
            },
            new TicketType
            {
                Id = ticket4Id, DefaultNetCost = 175.00m, MinimumPurchaseQuantity = 0,
                TicketKind = TicketKind.PerGroupOfFive, TourId = tour1Id
            }
        );
        // Create TourDestinations and link to Tours and Destinations

        var tourDestination1 = Guid.NewGuid();
        var tourDestination2 = Guid.NewGuid();
        var tourDestination3 = Guid.NewGuid();
        var tourDestination4 = Guid.NewGuid();
        context.TourDestinations.AddRange(
            new TourDestination
            {
                Id = tourDestination1, TourId = tour1Id, DestinationId = destination1Id,
                StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(16, 0, 0), SortOrder = 1, SortOrderByDate = 1
            }
            // ,
            // new TourDestination
            // {
            //     Id = tourDestination2, TourId = tour1Id, DestinationId = destination2Id,
            //     StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 30, 0), SortOrder = 2,
            //     SortOrderByDate = 2
            // },
            // new TourDestination
            // {
            //     Id = tourDestination3, TourId = tour1Id, DestinationId = destination3Id,
            //     StartTime = new TimeSpan(15, 30, 0), EndTime = new TimeSpan(16, 0, 0), SortOrder = 2,
            //     SortOrderByDate = 2
            // }
            // ,
            // new TourDestination
            // {
            //     Id = tourDestination4, TourId = tour1Id, DestinationId = destination4Id,
            //     StartTime = new TimeSpan(16, 0, 0), EndTime = new TimeSpan(17, 0, 0), SortOrder = 2, SortOrderByDate = 2
            // }
        );

        // Create TourSchedules and link to Tours
        var schedule1Id = Guid.NewGuid();
        var schedule2Id = Guid.NewGuid();

        context.TourSchedules.AddRange(
            new TourSchedule
            {
                Id = schedule1Id, TourId = tour1Id, OpenDate = DateTime.UtcNow, CloseDate = DateTime.UtcNow.AddDays(5),
                PriceChangeRate = 1.2, Remark = "No special remarks", CreatedAt = DateTime.UtcNow, CreatedBy = "admin",
                IsDeleted = false, 
            },
            new TourSchedule
            {
                Id = schedule2Id, TourId = tour1Id, OpenDate = DateTime.UtcNow.AddDays(10),
                CloseDate = DateTime.UtcNow.AddDays(15), PriceChangeRate = 1.5, Remark = "Special offer",
                CreatedAt = DateTime.UtcNow, CreatedBy = "admin", IsDeleted = false,
            }
        );
        context.TourScheduleTickets.AddRange(
            new TourScheduleTicket
            {
                TicketTypeId = ticket1Id, TourScheduleId = schedule2Id, NetCost = 65.00m, AvailableTicket = 20
            },
            new TourScheduleTicket
            {
                TicketTypeId = ticket2Id, TourScheduleId = schedule2Id, NetCost = 30.00m, AvailableTicket = 20
            },
            new TourScheduleTicket
            {
                TicketTypeId = ticket1Id, TourScheduleId = schedule1Id, NetCost = 75.00m, AvailableTicket = 20
            },

            new TourScheduleTicket
            {
                TicketTypeId = ticket2Id, TourScheduleId = schedule1Id, NetCost = 30.00m, AvailableTicket = 20
            },

            new TourScheduleTicket
            {
                TicketTypeId = ticket3Id, TourScheduleId = schedule1Id, NetCost = 100.00m, AvailableTicket = 20
            },

            new TourScheduleTicket
            {
                TicketTypeId = ticket4Id, TourScheduleId = schedule1Id, NetCost = 150.00m, AvailableTicket = 30
            }
        );
        AddImageFor(tour1Id);
        AddImageFor(tourDestination1);
        

        await context.SaveChangesAsync();
    }

    private void AddImageFor(Guid tourDestination1)
    {
        context.ImageUrls.AddRange(
            new ImageUrl()
            {
                Id = Guid.NewGuid(),
                RefId = tourDestination1,
                Url = "https://picsum.photos/500/300"
            },
            new ImageUrl()
            {
                Id = Guid.NewGuid(),
                RefId = tourDestination1,
                Url = "https://picsum.photos/500/300"
            },new ImageUrl()
            {
                Id = Guid.NewGuid(),
                RefId = tourDestination1,
                Url = "https://picsum.photos/500/300"
            },new ImageUrl()
            {
                Id = Guid.NewGuid(),
                RefId = tourDestination1,
                Url = "https://picsum.photos/500/300"
            },new ImageUrl()
            {
                Id = Guid.NewGuid(),
                RefId = tourDestination1,
                Url = "https://picsum.photos/500/300"
            }
        );
    }
}