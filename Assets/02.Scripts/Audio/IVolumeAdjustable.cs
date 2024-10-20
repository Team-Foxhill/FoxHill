using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxHill.Audio
{
    public interface IVolumeAdjustable
    {
        void OnVolumeChanged(float volume);
    }
}
