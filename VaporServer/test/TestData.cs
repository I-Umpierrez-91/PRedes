using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaporServer.Interfaces;

namespace VaporServer
{
    public static class TestData
    {
        public static void LoadTestData(ILogic _logic)
        {
            string workingDirectory = Environment.CurrentDirectory;

            if (workingDirectory.Contains("bin\\Debug\\net5.0"))
            {
                workingDirectory = workingDirectory.Replace("bin\\Debug\\net5.0", "test\\");
            }
            else
            {
                workingDirectory = workingDirectory + "\\test\\";
            }

            _logic.CreateGame("Paper Mario", "Arcade", "Sin ser uno de los grandes juegos de Nintendo, lo cierto es que este Paper Mario: The Origami King ha resultado ser un juego divertidísimo y con un encanto especial. Está lleno de buenos diálogos y chistes, las mazmorras molan un montón y los enfrentamientos con los jefes finales son tremendos. Que sus primeros pasos no te desanimen, el viaje vale mucho la pena.", workingDirectory + "PaperMario.jpeg");
            _logic.CreateGame("Microsoft Flight Simulator", "Simulador", "Los amantes de los simuladores de vuelo han estado de enhorabuena este año: Microsoft Flight Simulator es todo lo que se podía esperar de él y algo más, lo cual no está nada mal para una saga que lleva casi cuatro décadas ofreciendo algo que prácticamente nadie más ofrece.", workingDirectory + "FlightSimulator.jpeg");
            _logic.CreateGame("Wasteland 3", "RPG", "La comunidad tenía ganas de Wasteland 3, no hay más que ver las cifras que consiguió el juego durante su campaña de financiación colectiva: más de dos millones de dólares en diez horas y un total de 3,1 millones de dólares recaudados. Por suerte, más allá de la expectación y los números gordos, el nuevo RPG de ambientación postapocalíptica le salió muy bien a inXile. Un juego interesante, larguísimo y profundo que ningún fan del rol debería dejar pasar.", workingDirectory + "Wasteland3.jpeg");

            _logic.CreateUser("Jorge", "Jorge");
            _logic.CreateUser("Maria", "Maria");

            var PaperMarioId =_logic.GetGameId("Paper Mario");
            var FlightSimulatorId = _logic.GetGameId("Microsoft Flight Simulator");
            var Wastelandid = _logic.GetGameId("Wasteland 3");

            _logic.BuyGame("Jorge", PaperMarioId);
            _logic.BuyGame("Jorge", FlightSimulatorId);
            _logic.BuyGame("Maria", FlightSimulatorId);
            _logic.BuyGame("Maria", Wastelandid);

            _logic.ReviewGame("Jorge", PaperMarioId, "3", "Ta bueno");
            _logic.ReviewGame("Jorge", FlightSimulatorId, "5", "Me encanta este juego");
            _logic.ReviewGame("Maria", FlightSimulatorId, "4", "Esta demas, falta un poco de graficos");
            _logic.ReviewGame("Maria", Wastelandid, "1", "No me gustó, un bajon");

        }
    }
}
