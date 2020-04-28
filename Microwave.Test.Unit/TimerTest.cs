using System.Threading;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Unit
{
    [TestFixture]
    public class TimerTest
    {
        private Timer uut;

        [SetUp]
        public void Setup()
        {
            uut = new Timer();
        }

        [Test]
        public void Start_TimerTick_ShortEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.TimerTick += (sender, args) => pause.Set();
            // her testes om expired sker efter 2100ms. Da timer er sat til 2000 sekunder sker dette ikke. Ændret til 2
            uut.Start(2);
            // Rettet

            // wait for a tick, but no longer
            Assert.That(pause.WaitOne(1100));
        }

        [Test]
        public void Start_TimerTick_LongEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.TimerTick += (sender, args) => pause.Set();
            // her testes om expired sker efter 2100ms. Da timer er sat til 2000 sekunder sker dette ikke. Ændret til 2
            uut.Start(2);
            // Rettet

            // wait shorter than a tick, shouldn't come
            Assert.That(!pause.WaitOne(900));
        }

        [Test]
        public void Start_TimerExpires_ShortEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();
            // her testes om expired sker efter 2100ms. Da timer er sat til 2000 sekunder sker dette ikke. Ændret til 2
            uut.Start(2);
            // Rettet

            // wait for expiration, but not much longer, should come
            Assert.That(pause.WaitOne(2100));
        }

        [Test]
        public void Start_TimerExpires_LongEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();
            // Her vil test bestå da der testes om 2000 sekunder er expired på 1900ms. Det er dårligt test-design. Tid ændret til 2.
            uut.Start(2);
            // Rettet

            // wait shorter than expiration, shouldn't come
            Assert.That(!pause.WaitOne(1900));
        }

        [Test]
        public void Start_TimerTick_CorrectNumber()
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            int notifications = 0;

            uut.Expired += (sender, args) => pause.Set();
            uut.TimerTick += (sender, args) => notifications++;

            // I øjeblikket vil der gå 2000 sekunder før Expired kaldes. Derved er 2100ms ikke nok til at teste dette. Ændres til 2 sekunder.
            uut.Start(2);
            // Rettet

            // wait longer than expiration
            Assert.That(pause.WaitOne(2100));

            Assert.That(notifications, Is.EqualTo(2));
        }

        [Test]
        public void Stop_NotStarted_NoThrow()
        {
            Assert.That( () => uut.Stop(), Throws.Nothing);
        }

        [Test]
        public void Stop_Started_NoTickTriggered()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.TimerTick += (sender, args) => pause.Set();

            // her testes om expired sker efter 2100ms. Da timer er sat til 2000 sekunder sker dette ikke. Ændret til 2
            uut.Start(2);
            // Rettet

            uut.Stop();

            Assert.That(!pause.WaitOne(1100));
        }

        [Test]
        public void Stop_Started_NoExpiredTriggered()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();

            // her testes om expired sker efter 2100ms. Da timer er sat til 2000 sekunder sker dette ikke. Ændret til 2
            uut.Start(2);
            // Rettet

            uut.Stop();

            Assert.That(!pause.WaitOne(2100));
        }

        [Test]
        public void Stop_StartedOneTick_NoExpiredTriggered()
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            int notifications = 0;

            uut.Expired += (sender, args) => pause.Set();
            uut.TimerTick += (sender, args) => uut.Stop();

            uut.Start(2000);

            Assert.That(!pause.WaitOne(2100));
        }
    }
}