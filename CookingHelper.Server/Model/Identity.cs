using Microsoft.AspNetCore.Identity;

namespace CookingHelper.Model;

public class ApplicationUser : IdentityUser
{
    public string Permission { get; set; }
}
