using System;
using Microsoft.AspNetCore.Identity;

namespace HealthRec.Data;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {
        Id = new Guid(Guid.NewGuid().ToString());
    }
}