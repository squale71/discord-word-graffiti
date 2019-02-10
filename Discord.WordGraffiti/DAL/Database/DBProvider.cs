using System;

namespace Discord.WordGraffiti.DAL.Database
{
    public abstract class DBProvider : IDisposable
    {
        public abstract void Dispose();
    }
}
