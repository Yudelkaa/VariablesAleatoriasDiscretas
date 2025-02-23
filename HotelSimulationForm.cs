using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion
{
    public class HotelSimulationForm: Form
    {
        private Button startSimulationButton;
        private ListBox resultListBox;
        private PictureBox chartPictureBox;
        private Label statsLabel;
        private Label mostPopularRoomLabel;
        private List<Habitacion> habitaciones;
        private Random random = new Random();
        private System.Windows.Forms.Timer simulationTimer;
        private int step = 0;
        private int personasHospedadas = 0;
        private int personasNoHospedadas = 0; private Label finalResultsLabel;

        public HotelSimulationForm()
        {
            this.Text = "Simulación de Hotel";
            this.Size = new Size(600, 550);

            startSimulationButton = new Button() { Text = "Iniciar", Location = new Point(50, 10) };
            startSimulationButton.Click += StartSimulation;

            resultListBox = new ListBox() { Location = new Point(10, 50), Size = new Size(400, 450) };
            chartPictureBox = new PictureBox() { Location = new Point(500, 50), Size = new Size(400, 400) };
            statsLabel = new Label() { Location = new Point(50, 360), Size = new Size(560, 30) };
            mostPopularRoomLabel = new Label() { Location = new Point(5, 500), Size = new Size(560, 30) };
            finalResultsLabel = new Label() { Location = new Point(5, 530), Size = new Size(560, 30) };

            this.Controls.Add(startSimulationButton);
            this.Controls.Add(resultListBox);
            this.Controls.Add(chartPictureBox);
            this.Controls.Add(statsLabel);
            this.Controls.Add(mostPopularRoomLabel);
            this.Controls.Add(finalResultsLabel);

            InicializarHabitaciones();
            simulationTimer = new System.Windows.Forms.Timer();
            simulationTimer.Interval = 200; //para terminar en 20 segundos
            simulationTimer.Tick += SimulationStep;
        }

        private void InicializarHabitaciones()
        {
            habitaciones = new List<Habitacion>
            {
                new Habitacion { Numero = 1, Tipo = "Económica", Tamano = "Pequeña", Camas = 1 },
                new Habitacion { Numero = 2, Tipo = "Económica", Tamano = "Pequeña", Camas = 2 },
                new Habitacion { Numero = 3, Tipo = "Estándar", Tamano = "Mediana", Camas = 2 },
                new Habitacion { Numero = 4, Tipo = "Estándar", Tamano = "Mediana", Camas = 3 },
                new Habitacion { Numero = 5, Tipo = "Lujo", Tamano = "Grande", Camas = 4 }
            };
        }

        private void StartSimulation(object sender, EventArgs e)
        {
            step = 0;
            resultListBox.Items.Clear();
            personasHospedadas = 0;
            personasNoHospedadas = 0;
            simulationTimer.Start();
        }

        private void SimulationStep(object sender, EventArgs e)
        {
            if (step >= 100)
            {
                simulationTimer.Stop();
                MostrarResultados();
                DibujarGrafico();
                MostrarHabitacionMasSolicitada();
                return;
            }

            Persona persona = GenerarPersona();

            Habitacion habitacion = AsignarHabitacion(persona);
            if (habitacion != null)
            {
                habitacion.VecesOcupada++;
                personasHospedadas++;
                resultListBox.Items.Add($"Huésped de {persona.GrupoPersonas} personas con preferencia {persona.PreferenciaTamano} y presupuesto {persona.Presupuesto} asignado a habitación {habitacion.Numero} ({habitacion.Tipo}, {habitacion.Camas} camas)");
            }
            else
            {
                personasNoHospedadas++;
                resultListBox.Items.Add($"No se pudo asignar habitación al huésped de {persona.GrupoPersonas} personas");
            }

            step++;
        }

        private Persona GenerarPersona()
        {
            return new Persona
            {
                GrupoPersonas = random.Next(1, 5),
                PreferenciaTamano = random.NextDouble() < 0.5 ? "Pequeña" : "Mediana",
                Presupuesto = random.NextDouble() < 0.7 ? "Económica" : "Estándar"
            };
        }

        private Habitacion AsignarHabitacion(Persona persona)
        {
            var habitacionesDisponibles = habitaciones
                .Where(h => h.Tamano == persona.PreferenciaTamano && h.Tipo.Contains(persona.Presupuesto))
                .ToList();

            if (habitacionesDisponibles.Any())
            {
                return habitacionesDisponibles[random.Next(habitacionesDisponibles.Count)];
            }

            return null;
        }

        private void MostrarResultados()
        {
            resultListBox.Items.Add("\nResultados finales:");
            foreach (var habitacion in habitaciones)
            {
                resultListBox.Items.Add($"Habitación {habitacion.Numero} ({habitacion.Tipo} - {habitacion.Camas} camas): {habitacion.VecesOcupada} usos");
            }

            resultListBox.Items.Add($"\nResumen de la simulación:");
            resultListBox.Items.Add($"Número de personas hospedadas: {personasHospedadas}");
            resultListBox.Items.Add($"Número de personas no hospedadas: {personasNoHospedadas}");
        }
        private void DibujarGrafico()
        {
            Bitmap bitmap = new Bitmap(chartPictureBox.Width, chartPictureBox.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);

            int barWidth = chartPictureBox.Width / habitaciones.Count;
            int maxOcupaciones = habitaciones.Max(h => h.VecesOcupada);

            for (int i = 0; i < habitaciones.Count; i++)
            {
                int barHeight = (habitaciones[i].VecesOcupada * chartPictureBox.Height) / (maxOcupaciones + 1);
                Brush color = habitaciones[i].Tipo == "Económica" ? Brushes.Green : (habitaciones[i].Tipo == "Estándar" ? Brushes.Orange : Brushes.Red);
                g.FillRectangle(color, i * barWidth, chartPictureBox.Height - barHeight, barWidth - 5, barHeight);
            }

            chartPictureBox.Image = bitmap;
        }

        private void MostrarHabitacionMasSolicitada()
        {
            var habitacionMasSolicitada = habitaciones.OrderByDescending(h => h.VecesOcupada).First();
            mostPopularRoomLabel.Text = $"Habitación más solicitada: Habitación {habitacionMasSolicitada.Numero} ({habitacionMasSolicitada.Tipo}, {habitacionMasSolicitada.Camas} camas), " +
                $"con {habitacionMasSolicitada.VecesOcupada} usos.";
        }


    }
}
