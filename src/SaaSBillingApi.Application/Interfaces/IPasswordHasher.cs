using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SaaSBillingApi.Application.Interfaces;

namespace SaaSBillingApi.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword (string password);
        bool VerifyPassword(string passowrdHash, string providedPassword);

    }
}
