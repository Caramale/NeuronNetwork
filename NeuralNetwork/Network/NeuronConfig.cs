using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Network
{
    public class NeuronConfig
    {

        #region fields
        private double _currentOutput;
        private double _input;
        private Weight[] _weights;
        private int _patternInpitPosition;
        private bool _isOutputNeuron;
        #endregion


        #region proporties
        public double CurrentOutput
        {
            get { return _currentOutput; }
        }

        public Weight[] Weights
        {
            get { return _weights; }
        }

        public double Input
        {
            get { return _input; }
        }

        public int PatternInpitPosition
        {
            get { return _patternInpitPosition; }
        }

        public bool IsOutputNeuron
        {
            get { return _isOutputNeuron; }
        }
        #endregion


        #region constructors
        public NeuronConfig()
        {
            _input = 0;
            _isOutputNeuron = false;
        }
        #endregion
    }
}
