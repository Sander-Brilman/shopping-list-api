using System.ComponentModel.DataAnnotations;

namespace MixPlannerApi.Data.Models;

public sealed class TaskItemDBEntity
{
    [Key]
    public int Id { get; set; }

    [MaxLength(150)]
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";


    public int Order { get; set; } = 1;


    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }


    public int TimesCompleted { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedOnUTC { get; set; }


    public TaskListDBEntity? List { get; set; }
    public int ListId { get; set; }

    
    public int? ParentItemId { get; set; }
    public List<TaskItemDBEntity> SubTasks { get; set; } = [];
}
