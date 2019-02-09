using System;

namespace Discord.WordGraffiti.Models.Database
{
    public abstract class DbConnection : IDisposable
    {
        public abstract void Dispose();
    }
}
