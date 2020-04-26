using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NUnit.Framework;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MicrowaveOven.Test.Integration
{
    [TestFixture]

    public class IT1_Button_Door_UserInterface
    {
        private UserInterface _userInterface;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDoor _door;
        private IDisplay _display;
        private ILight _light;
        private ICookController _cooker;

        [SetUp]

        public void SetUp()
        {
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _display = Substitute.For<IDisplay>();
            _light = Substitute.For<ILight>();
            _cooker = Substitute.For<ICookController>();
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cooker);

        }

        #region Skridt 1-5

        /// <summary>
        /// Skridt 1-2 i UC
        /// </summary>
        [Test]
        public void DoorOpen_LightOn()
        {
            _door.Open();

            _light.Received().TurnOn();
        }

        /// <summary>
        /// Skridt 4-5 i UC
        /// </summary>
        [Test]
        public void DoorOpen_DoorClose_LightOff()
        {
            _door.Open();
            _door.Close();

            _light.Received().TurnOff();
        }

        #endregion

        #region Skridt 6 + Extensions 

        /// <summary>
        /// Skridt 6
        /// </summary>
        /// <param name="numberOfPresses"></param>
        /// <param name="powerLevel"></param>
        [TestCase(1, 50, 1)]
        [TestCase(4, 200, 1)]
        [TestCase(13, 650, 1)]
        [TestCase(14, 700, 1)]
        [TestCase(15, 50, 2)]
        [TestCase(20, 300, 2)]
        public void PowerButtonPress_PowerDisplayed(int numberOfPresses, int powerLevel, int numberOfTimesCalled)
        {
            _door.Open();
            _door.Close();

            for (int i = 0; i < numberOfPresses; i++)
            {
                if (i == numberOfPresses - 1)
                {
                    _display.ClearReceivedCalls();
                }
                _powerButton.Press();
            }

            _display.Received(numberOfTimesCalled).ShowPower(Arg.Is<int>(powerLevel));

        }


        /// <summary>
        /// Extension 1: Skridt 1
        /// </summary>
        /// <param name="numberOfPresses"></param>
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(13)]
        [TestCase(14)]
        public void WhileSettingPower_StartCancelButtonPress_DisplayIsCleared(int numberOfPresses)
        {
            _door.Open();
            _door.Close();

            for (int i = 0; i < numberOfPresses; i++)
            {
                _powerButton.Press();
            }

            _startCancelButton.Press();

            _display.Received().Clear();
        }

        /// <summary>
        /// Extension 2: Skridt 4 - afbrudt setup under power indstilling
        /// </summary>
        [Test]
        public void WhileSettingPower_DoorOpened_LightOn()
        {
            _powerButton.Press();
            _door.Open();
            
            _light.Received(1).TurnOn();
        }

        /// <summary>
        /// Extension 2: Skridt 5 - afbrudt setup under power indstilling
        /// </summary>

        [Test]
        public void WhileSettingPower_DoorOpened_DisplayIsCleared()
        {
            _powerButton.Press();
            _door.Open();

            _display.Received(1).Clear();
        }

        #endregion

        #region Skridt 7 + Extensions

        /// <summary>
        /// Skridt 7
        /// </summary>
        /// <param name="numberOfPresses"></param>
        /// <param name="powerLevel"></param>
        [TestCase(1, 1)]
        [TestCase(4, 4)]
        [TestCase(13, 13)]
        [TestCase(14, 14)]
        [TestCase(15, 15)]
        [TestCase(20, 20)]
        public void TimeButtonPress_TimeDisplayed(int numberOfPresses, int time)
        {
            _door.Open();
            _door.Close();
            _powerButton.Press();

            for (int i = 0; i < numberOfPresses; i++)
            {
                if (i == numberOfPresses - 1)
                {
                    _display.ClearReceivedCalls();
                }
                _timeButton.Press();
            }

            _display.Received().ShowTime(Arg.Is<int>(time),Arg.Is<int>(0));

        }


        /// <summary>
        /// Extension 2: Skridt 4 - afbrudt setup under time indstilling
        /// </summary>
        [Test]
        public void WhileSettingTime_DoorOpened_LightOn()
        {
            _powerButton.Press();
            _timeButton.Press();
            _door.Open();
            
            _light.Received(1).TurnOn();
        }

        /// <summary>
        /// Extension 2: Skridt 5 - afbrudt setup under time indstilling
        /// </summary>

        [Test]
        public void WhileSettingTime_DoorOpened_DisplayIsCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _door.Open();

            _display.Received(1).Clear();
        }

        #endregion

        #region Skridt 8-9

        /// <summary>
        /// Skridt 8
        /// </summary>
        /// <param name="numberOfPowerButtonPresses"></param>
        /// <param name="numberOfTimeButtonPresses"></param>
        /// <param name="power"></param>
        /// <param name="time"></param>

        [TestCase(1, 1,50,60)]
        [TestCase(4, 3, 200, 180)]
        [TestCase(13, 17, 650, 1020)]
        [TestCase(14, 22, 700, 1320)]

        public void CookingIsStarted(int numberOfPowerButtonPresses, int numberOfTimeButtonPresses, int power, int time)
        {
            for (int i = 0; i < numberOfPowerButtonPresses; i++)
            {
                _powerButton.Press();
            }
            for (int i = 0; i < numberOfTimeButtonPresses; i++)
            {
                _timeButton.Press();
            }

            _startCancelButton.Press();

            _cooker.Received().StartCooking(power,time);
        }

        /// <summary>
        /// Skridt 9
        /// </summary>
        [Test]

        public void CookingHasStarted_LightIsOn()
        {
            _cooker.StartCooking(100,60);
            _light.Received().TurnOn();
        }

        #endregion


        //Hører til i trin 3

        ///// <summary>
        ///// Extension 1: Skridt 2
        ///// </summary>
        ///// <param name="numberOfPresses"></param>
        //[TestCase(1)]
        //[TestCase(4)]
        //[TestCase(13)]
        //[TestCase(14)]
        //public void StartCancelButtonPress_WhileSettingPower_ValuesReset(int numberOfPresses)
        //{
        //    _door.Open();
        //    _door.Close();

        //    for (int i = 0; i < numberOfPresses; i++)
        //    {
        //        _powerButton.Press();
        //    }

        //    _startCancelButton.Press();

        //    _powerButton.Press();

        //    output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"Display shows: 50 W")));
        //}

    }
}
