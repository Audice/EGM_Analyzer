using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGM_Analyzer.EventsArgs
{
    public enum LoadingState
    {
        StartLoading,
        EndLoading,
        Loading
    } 
    public class LoadingStateEventArgs
    {
        public LoadingState State {  get; set; }      

    }
}
