using System.ComponentModel.DataAnnotations;

namespace AdminLte.ViewModels
{
    public class RoleModel
    {
        [Required , StringLength(256)]
        public string Name { get; set; }
    }
}
