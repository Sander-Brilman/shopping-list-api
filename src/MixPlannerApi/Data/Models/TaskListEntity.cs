using System.ComponentModel.DataAnnotations;

namespace MixPlannerApi.Data.Models;

public sealed class TaskListDBEntity
{
    [Key]
    public int Id { get; set; }

    [MaxLength(150)]
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    [MaxLength(9)]
    public string? Color { get; set; } = null;

    public List<TaskItemDBEntity> Items { get; set; } = [];

    public List<TaskListDBEntity> SubLists = [];

    public TaskListDBEntity? ParentList { get; set; }

    public int? ParentListId { get; set; } 
}
