using System.ComponentModel.DataAnnotations;

namespace AprilBookStore.Web.ViewModels.User;

public class CreateRoleViewModel
{
    [Required, Display(Name = "Role Name")]
    public string RoleName { get; set; }
}
