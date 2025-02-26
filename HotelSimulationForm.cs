using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion
{
    public class HotelSimulationForm : Form
    {
        private Button startSimulationButton;
        private ListBox resultListBox;
        private PictureBox chartPictureBox;
        private Label statsLabel;
        private Label dineroGanadoLabel;
        private Label mostPopularRoomLabel;
        private Label finalResultsLabel;
        private Label lostRevenueLabel;
        private List<Habitacion> habitaciones;
        private Random random = new Random();
        private System.Windows.Forms.Timer simulationTimer;
        private int step = 0;
        private int personasHospedadas = 0;
        private int personasNoHospedadas = 0;
        private double perdidaIngresos = 0;
        private double dineroGanado = 0;
        private List<int> habitacionesOcupadasHoy = new List<int>();
 
        private int cantidadEconómicas = 40;
        private int cantidadEstándar = 40;
        private int cantidadLujo = 20;
        public HotelSimulationForm()
        {
            this.Text = "Simulación de Hotel";
            this.Size = new Size(1200, 700);

            startSimulationButton = new Button() { Text = "Iniciar", Location = new Point(50, 10), Size = new Size(100, 30) };
            startSimulationButton.Click += StartSimulation;

            resultListBox = new ListBox() { Location = new Point(10, 50), Size = new Size(850, 550) };
            chartPictureBox = new PictureBox() { Location = new Point(870, 50), Size = new Size(300, 250) };
            statsLabel = new Label() { Location = new Point(10, 610), Size = new Size(1140, 30), AutoSize = true };
            mostPopularRoomLabel = new Label() { Location = new Point(10, 640), Size = new Size(1140, 30), AutoSize = true };
            finalResultsLabel = new Label() { Location = new Point(10, 670), Size = new Size(1140, 30), AutoSize = true };
            lostRevenueLabel = new Label() { Location = new Point(10, 700), Size = new Size(1140, 30), AutoSize = true };
            dineroGanadoLabel = new Label() { Location = new Point(10, 730), Size = new Size(1140, 30), AutoSize = true };

            this.Controls.Add(startSimulationButton);
            this.Controls.Add(resultListBox);
            this.Controls.Add(chartPictureBox);
            this.Controls.Add(statsLabel);
            this.Controls.Add(mostPopularRoomLabel);
            this.Controls.Add(finalResultsLabel);
            this.Controls.Add(lostRevenueLabel);
            this.Controls.Add(dineroGanadoLabel);

            InicializarHabitaciones();
            simulationTimer = new System.Windows.Forms.Timer();
            simulationTimer.Interval = 200;
            simulationTimer.Tick += PasosSimulacion;
        }

        private void InicializarHabitaciones()
        {
            habitaciones = new List<Habitacion>();

            for (int i = 1; i <= cantidadEconómicas; i++)
            {
                habitaciones.Add(new Habitacion(i, "Económica", "Pequeña", 1, false, false, false) { Precio = 3000 });
            }

            for (int i = 1; i <= cantidadEstándar; i++)
            {
                habitaciones.Add(new Habitacion(i + cantidadEconómicas, "Estándar", "Mediana", 2, false, true, false) { Precio = 5500 });
            }

            for (int i = 1; i <= cantidadLujo; i++)
            {
                habitaciones.Add(new Habitacion(i + cantidadEconómicas + cantidadEstándar, "Lujo", "Grande", 4, true, false, true) { Precio = 12000 });
            }

            AsignarCaracteristicas();
        }

        private void AsignarCaracteristicas()
        {
            int vistaAlMarCount = (int)(habitaciones.Count * 0.4); // 40% 
            int cercaDelAscensorCount = (int)(habitaciones.Count * 0.6); // 60% 
            int conBalconCount = (int)(habitaciones.Count * 0.5); // 50% 

            for (int i = 0; i < vistaAlMarCount; i++)
            {
                habitaciones[i].TieneVistaAlMar = true;
            }

            for (int i = 0; i < cercaDelAscensorCount; i++)
            {
                habitaciones[i].CercaAscensor = true;
            }

            for (int i = 0; i < conBalconCount; i++)
            {
                habitaciones[i].TieneBalcon = true;
            }
        }

        private void StartSimulation(object sender, EventArgs e)
        {
            step = 0;
            resultListBox.Items.Clear();
            personasHospedadas = 0;
            personasNoHospedadas = 0;
            perdidaIngresos = 0;
            simulationTimer.Start();
        }

        private void PasosSimulacion(object sender, EventArgs e)
        {
            if (step >= 100)
            {
                simulationTimer.Stop();
                MostrarResultados();
                MostrarHabitacionMasSolicitada();
                return;
            }

            Persona persona = GenerarPersona();
            Habitacion habitacion = AsignarHabitacion(persona);
            if (habitacion != null)
            {
                habitacion.VecesOcupada++;
                personasHospedadas++;
                dineroGanado += habitacion.Precio;
                resultListBox.Items.Add($"Huésped de {persona.GrupoPersonas} personas con preferencia {persona.PreferenciaTamano} y presupuesto {persona.Presupuesto} asignado a habitación {habitacion.Numero} ({habitacion.Tipo}, {habitacion.Camas} camas) por ${habitacion.Precio}");
            }
            else
            {
                personasNoHospedadas++;
                perdidaIngresos += 100;
                resultListBox.Items.Add($"No se pudo asignar habitación al huésped de {persona.GrupoPersonas} personas");
            }
            dineroGanadoLabel.Text = $"Dinero ganado por hospedar: ${dineroGanado}";

            step++;
        }

        private Habitacion AsignarHabitacion(Persona persona)
        {

            double probabilidadNoHospedarse = persona.Presupuesto == "Económica" ? 0.4 : persona.Presupuesto == "Estándar" ? 0.2 : 0.1;

            if (random.NextDouble() < probabilidadNoHospedarse)
            {
                personasNoHospedadas++;
                return null;
            }

            var habitacionesDisponibles = habitaciones
                .Where(h => h.Tipo == persona.Presupuesto && !habitacionesOcupadasHoy.Contains(h.Numero))
                .ToList();

            if (!habitacionesDisponibles.Any())
            {
                return null;
            }

            var habitacion = habitacionesDisponibles.First();
            habitacionesOcupadasHoy.Add(habitacion.Numero);
            habitacion.VecesOcupada++;
            dineroGanado += habitacion.Precio;
            return habitacion;
        }

        private Persona GenerarPersona()
        {
            double rand = random.NextDouble();
            string presupuesto;

            if (rand < 0.7)
                presupuesto = "Económica";  // 70% de probabilidad
            else if (rand < 0.9)
                presupuesto = "Estándar";   // 20% de probabilidad
            else
                presupuesto = "Lujo";       // 10% de probabilidad

            return new Persona
            {
                GrupoPersonas = random.Next(1, 5),
                PreferenciaTamano = random.NextDouble() < 0.5 ? "Pequeña" : "Mediana",
                Presupuesto = presupuesto
            };
        }

        private void MostrarHabitacionMasSolicitada()
        {
            var habitacionMasSolicitada = habitaciones.OrderByDescending(h => h.VecesOcupada).FirstOrDefault();
            if (habitacionMasSolicitada != null)
            {
                mostPopularRoomLabel.Text = $"Habitación más solicitada: {habitacionMasSolicitada.Numero} ({habitacionMasSolicitada.Tipo} - {habitacionMasSolicitada.Camas} camas)";
            }
            else
            {
                mostPopularRoomLabel.Text = "No se registraron habitaciones ocupadas.";
            }
        }

        private void MostrarResultados()
        {
            resultListBox.Items.Add("\nResultados finales:");
            foreach (var habitacion in habitaciones)
            {
                resultListBox.Items.Add($"Habitación {habitacion.Numero} ({habitacion.Tipo} - {habitacion.Camas} camas): {habitacion.VecesOcupada} usos");
            }

            finalResultsLabel.Text = $"Número de personas hospedadas: {personasHospedadas} | Número de personas no hospedadas: {personasNoHospedadas}";
            lostRevenueLabel.Text = $"Dinero perdido por no hospedar: ${perdidaIngresos}";
            dineroGanadoLabel.Text = $"Dinero ganado por hospedar: ${dineroGanado}";
        }
    }
}
