using R3;
using UnityEngine;

namespace MotorHome.Scripts
{
    public interface IDialogShower
    {
        Observable<Unit> Show(string[] phrases);
        //void Close();
        //void Open();
        void Click();
        void Clear();
    }
}