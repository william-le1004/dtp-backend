using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos;
    public record TourResponse(Guid Id, string Title, Guid? CompanyId, Guid? Category, string? Description);

