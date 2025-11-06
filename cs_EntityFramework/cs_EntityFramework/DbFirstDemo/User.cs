using System;
using System.Collections.Generic;

namespace cs_EntityFramework.DbFirstDemo;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
