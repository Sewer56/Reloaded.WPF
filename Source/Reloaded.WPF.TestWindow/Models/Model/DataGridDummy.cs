using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reloaded.WPF.MVVM;

namespace Reloaded.WPF.TestWindow.Models.Model
{
    /// <summary>
    /// A dummy class for representing items on a EnhancedDataGrid
    /// </summary>
    public class DataGridDummy : ObservableObject
    {
        public DummyType Type { get; set; }
        public bool IsAlive { get; set; }
        public string Name { get; set; }
        public int WeaponId { get; set; }
        public float WeaponRange { get; set; }

        public enum DummyType
        {
            Human,
            Virtual,
            God
        }
    }
}
