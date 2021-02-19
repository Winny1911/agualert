using System;

namespace TaVazando
{
	public interface IGSMSignal
	{
		int SignalStrength {get;set;}
		void GetSignalStrength();
	}
}

