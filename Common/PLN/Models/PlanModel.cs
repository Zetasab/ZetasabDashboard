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
        public PlanMode PlanMode { get; set; }
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
                _ => Icons.Material.Filled.HelpOutline
            };
        }

        public Color GetModeColor() => PlanMode switch
        {
            PlanMode.Chill => Color.Info,     // azul relajado
            PlanMode.Casa => Color.Success,  // verde "home"
            PlanMode.Salir => Color.Warning,  // ámbar enérgico
            PlanMode.Planificar => Color.Primary,  // color de marca/acción
            _ => Color.Default
        };

        public string GetModeHex() => PlanMode switch
        {
            PlanMode.Chill => "#29B6F6", // Light Blue 400
            PlanMode.Casa => "#66BB6A", // Green 400
            PlanMode.Salir => "#FFCA28", // Amber 400
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
        Chill,
        Casa,
        Salir,
        Planificar
    }
    
}
