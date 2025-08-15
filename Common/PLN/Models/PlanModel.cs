using MudBlazor;
using System;
using System.Linq;
using static MongoDB.Driver.WriteConcern;

namespace ZetaDashboard.Common.PLN.Models
{
    public class PlanModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        public PlanStatus Status { get; set; } = PlanStatus.Pending;
        public DateTime? Date { get; set; } = DateTime.Today;
        public string Place { get; set; }
        public int Score { get; set; } = -1;
        public PlanMode PlanMode { get; set; } = PlanMode.Casa;
        public PlanModel()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string GetModeIcon()
        {
            return PlanMode switch
            {
                PlanMode.Chill => Icons.Material.Filled.SelfImprovement,
                PlanMode.Casa => Icons.Material.Filled.Home,
                PlanMode.Salir => Icons.Material.Filled.Nightlife,
                PlanMode.Planificar => Icons.Material.Filled.EditCalendar,
                PlanMode.Comidas => Icons.Material.Filled.Fastfood,
                _ => Icons.Material.Filled.HelpOutline
            };
        }

        public Color GetModeColor() => PlanMode switch
        {
            PlanMode.Chill => Color.Info,     // azul relajado
            PlanMode.Casa => Color.Success,  // verde "home"
            PlanMode.Comidas => Color.Warning,  // ámbar enérgico
            PlanMode.Salir => Color.Tertiary,  // ámbar enérgico
            PlanMode.Planificar => Color.Primary,  // color de marca/acción
            _ => Color.Default
        };

        public string GetModeHex() => PlanMode switch
        {
            PlanMode.Chill => "#29B6F6", // Light Blue 400
            PlanMode.Casa => "#66BB6A", // Green 400
            PlanMode.Comidas => "#FFCA28", // Amber 400
            PlanMode.Salir => "#b424e0", 
            PlanMode.Planificar => "#7C4DFF", // Deep Purple A200
            _ => "#90A4AE"  // Blue Grey 300 (neutral)
        };
    }
    public enum PlanStatus
    {
        Pending,
        Planned,
        Done,
        Cancelled
    }
    public enum PlanMode
    {
        None,
        Chill,
        Casa,
        Comidas,
        Salir,
        Planificar
    }
    
}
