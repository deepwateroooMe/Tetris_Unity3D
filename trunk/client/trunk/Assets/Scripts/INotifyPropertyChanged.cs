using System.ComponentModel;

namespace tetris3d {

    public interface INotifyPropertyChanged {
    
        event PropertyChangedEventHandler PropertyChanged;
    }
}