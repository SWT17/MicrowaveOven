﻿using System;
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

        #region Skridt 1-5 / 15-19

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
        [TestCase(1, 50)]
        [TestCase(4, 200)]
        [TestCase(13, 650)]
        [TestCase(14, 700)]
        public void PowerButtonPress_PowerDisplayed(int numberOfPresses, int powerLevel)
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

            _display.Received(1).ShowPower(Arg.Is<int>(powerLevel));

        }

        [TestCase(15, 50, 2)]
        [TestCase(20, 300, 2)]
        public void PowerButtonPress_MoreThan15Times_PowerDisplayed(int numberOfPresses, int powerLevel, int numberOfTimesCalled)
        {
            _door.Open();
            _door.Close();

            for (int i = 0; i < numberOfPresses; i++)
            {
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
        /// Extension 1: Skridt 2
        /// </summary>
        /// <param name="numberOfPresses"></param>
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(13)]
        public void WhileSettingPower_StartCancelButtonPress_PowerSettingCleared(int numberOfPresses)
        {
            _door.Open();
            _door.Close();

            for (int i = 0; i < numberOfPresses; i++)
            {
                _powerButton.Press();
            }

            _startCancelButton.Press();

            _powerButton.Press();

            _display.Received().ShowPower(Arg.Is<int>(50));
        }

        /// <summary>
        /// Extension 2: Skridt 4
        /// </summary>
        [Test]
        public void WhileSettingPower_DoorOpened_LightOn()
        {
            _powerButton.Press();
            _door.Open();

            _light.Received(1).TurnOn();
        }

        /// <summary>
        /// Extension 2: Skridt 5
        /// </summary>

        [Test]
        public void WhileSettingPower_DoorOpened_DisplayIsCleared()
        {
            _powerButton.Press();
            _door.Open();

            _display.Received(1).Clear();
        }

        /// <summary>
        /// Extension 2: Skridt 6
        /// </summary>

        [Test]
        public void WhileSettingPower_DoorOpened_PowerSettingCleared()
        {
            _powerButton.Press();
            _door.Open();

            _powerButton.Press();

            _display.Received().ShowPower(Arg.Is<int>(50));
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

            _display.Received().ShowTime(Arg.Is<int>(time), Arg.Is<int>(0));

        }


        /// <summary>
        /// Extension 2: Skridt 4
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
        /// Extension 2: Skridt 5
        /// </summary>

        [Test]
        public void WhileSettingTime_DoorOpened_DisplayIsCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _door.Open();

            _display.Received(1).Clear();
        }

        /// <summary>
        /// Extension 2: Skridt 6
        /// </summary>

        [TestCase(1)]
        [TestCase(4)]
        [TestCase(13)]
        public void WhileSettingPower_DoorOpened_TimeSettingCleared(int numberOfPresses)
        {
            _powerButton.Press();
            for (int i = 0; i < numberOfPresses; i++)
            {
                _timeButton.Press();

            }
            _door.Open();

            _powerButton.Press();
            _timeButton.Press();

            _display.Received().ShowTime(Arg.Is<int>(1), Arg.Is<int>(0));
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

        [TestCase(1, 1, 50, 60)]
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

            _cooker.Received().StartCooking(power, time);
        }

        /// <summary>
        /// Skridt 9
        /// </summary>
        [Test]

        /// <summary>
        /// Test burde fejle, cooker starter ikke at lyset tændes, det er startCancelButtonPressed der skal tænde lyset. 
        /// </summary>
        public void CookingHasStarted_LightIsOn() 
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _light.Received().TurnOn();
        }

        #endregion

        #region Extension 3

        /// <summary>
        /// Extension 3: Skridt 8
        /// </summary>
        [Test]

        public void CookingHasStarted_StartCancelButtonPressed_CookerIsStopped()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _cooker.Received().Stop();
        }

        /// <summary>
        /// Extension 3: Skridt 9
        /// </summary>
        [Test]

        public void CookingHasStarted_StartCancelButtonPressed_DisplayIsCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _display.Received().Clear();
        }

        /// <summary>
        /// Extension 3: Skridt 10
        /// </summary>
        [Test]

        public void CookingHasStarted_StartCancelButtonPressed_LightIsOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _light.Received().TurnOff();
        }
        /// <summary>
        /// Extension 3: Skridt 11
        /// </summary>
        [Test]

        public void CookingHasStarted_StartCancelButtonPressed_PowerSettingCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _powerButton.Press();

            _display.Received().ShowPower(Arg.Is<int>(50));
        }

        /// <summary>
        /// Extension 3: Skridt 11
        /// </summary>
        [Test]

        public void CookingHasStarted_StartCancelButtonPressed_TimeSettingCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _powerButton.Press();
            _timeButton.Press();

            _display.Received().ShowTime(Arg.Is<int>(1), Arg.Is<int>(0));
        }
        #endregion

        #region Extension 4

        /// <summary>
        /// Extension 4: Skridt 13
        /// </summary>
        [Test]
        public void CookingHasStarted_DoorOpened_CookerIsStopped()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _door.Open();

            _cooker.Received().Stop();
        }

        /// <summary>
        /// Extension 4: Skridt 14
        /// </summary>
        [Test]

        public void CookingHasStarted_DoorOpened_DisplayIsCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _door.Open();

            _display.Received().Clear();
        }

        /// <summary>
        /// Extension 4: Skridt 15
        /// </summary>
        [Test]

        public void CookingHasStarted_DoorOpened_PowerSettingCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _door.Open();

            _powerButton.Press();

            _display.Received().ShowPower(Arg.Is<int>(50));
        }

        /// <summary>
        /// Extension 4: Skridt 15
        /// </summary>
        [Test]

        public void CookingHasStarted_DoorOpened_TimeSettingCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _door.Open();

            _powerButton.Press();
            _timeButton.Press();

            _display.Received().ShowTime(Arg.Is<int>(1), Arg.Is<int>(0));
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
