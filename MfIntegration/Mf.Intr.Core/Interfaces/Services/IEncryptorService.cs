using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Services;
public interface IEncryptorService
{
    string EncryptString(string text, byte[] key);
    string DecryptString(string text, byte[] key);
    bool IsEncrypted(string text);
}
