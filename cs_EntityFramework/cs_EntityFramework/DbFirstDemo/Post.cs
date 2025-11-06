using System;
using System.Collections.Generic;

namespace cs_EntityFramework.DbFirstDemo;

public partial class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
