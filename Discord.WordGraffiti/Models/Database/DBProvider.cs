using System;

namespace Discord.WordGraffiti.Models.Database
{
    public abstract class DBProvider : IDisposable
    {
        public abstract void Dispose();
    }
}
