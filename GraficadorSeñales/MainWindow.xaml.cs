using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraficadorSeñales
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mostrarSegundaSeñal(false);
        }

        private void BtnGraficar_Click(object sender, RoutedEventArgs e)
        {

            double tiempoInicial = double.Parse(txtbox_tiempoInicial.Text);
            double tiempoFinal = double.Parse(txtbox_tiempoFinal.Text);
            double muestro = double.Parse(txtbox_muestreo.Text);

            Señal señal;
            Señal segundaSeñal = null;
            Señal señalResultante;

            switch (CbTipoSeñal.SelectedIndex)
            {
                case 0: //Parabolica
                    señal = new SeñalParabolica();
                    break;
                case 1: //Senoidal
                    double amplitud =
                        double.Parse(
                    ((ConfiguracionSeñalSenoidal)(PanelConfiguracion.Children[0])).txtbox_amplitud.Text
                    );

                    double fase =
                        double.Parse(
                    ((ConfiguracionSeñalSenoidal)(PanelConfiguracion.Children[0])).txtbox_fase.Text
                    );

                    double frecuencia =
                        double.Parse(
                    ((ConfiguracionSeñalSenoidal)(PanelConfiguracion.Children[0])).txtbox_frecuencia.Text
                    );

                    señal = new SeñalSenoidal(amplitud, fase, frecuencia);
                    break;
                case 2: //Exponencial
                    double alpha =
                        double.Parse(
                            ((ConfiguracionSeñalExponencial)(PanelConfiguracion.Children[0])).txt_alpha.Text
                    );
                    señal = new SeñalExponencial(alpha);
                    break;
                case 3: //Audio
                    string rutaArchivo = ((ConfiguracionSeñalAudio)(PanelConfiguracion.Children[0])).txtRutaArchivo.Text;
                    señal = new SeñalAudio(rutaArchivo);
                    txtbox_tiempoInicial.Text = señal.TiempoInicial.ToString();
                    txtbox_tiempoFinal.Text = señal.TiempoFinal.ToString();
                    txtbox_muestreo.Text = señal.FrecuenciaMuestreo.ToString();
                    break;
                default:
                    señal = null;
                    break;
            }

            if (CbTipoSeñal.SelectedIndex != 3)
            {
                señal.TiempoInicial = tiempoInicial;
                señal.TiempoFinal = tiempoFinal;
                señal.FrecuenciaMuestreo = muestro;

                señal.construirSeñal();
            }

            //Construir segunda señal si es necesario
            if(CbOperacion.SelectedIndex == 2)
            {
                switch (CbTipoSeñal_2.SelectedIndex)
                {
                    case 0: //Parabolica
                        segundaSeñal = new SeñalParabolica();
                        break;
                    case 1: //Senoidal
                        double amplitud =
                            double.Parse(
                        ((ConfiguracionSeñalSenoidal)(PanelConfiguracion_2.Children[0])).txtbox_amplitud.Text
                        );

                        double fase =
                            double.Parse(
                        ((ConfiguracionSeñalSenoidal)(PanelConfiguracion_2.Children[0])).txtbox_fase.Text
                        );

                        double frecuencia =
                            double.Parse(
                        ((ConfiguracionSeñalSenoidal)(PanelConfiguracion_2.Children[0])).txtbox_frecuencia.Text
                        );

                        segundaSeñal = new SeñalSenoidal(amplitud, fase, frecuencia);
                        break;
                    case 2: //Exponencial
                        double alpha =
                            double.Parse(
                                ((ConfiguracionSeñalExponencial)(PanelConfiguracion_2.Children[0])).txt_alpha.Text
                        );
                        señal = new SeñalExponencial(alpha);
                        break;
                    case 3: //Audio
                        string rutaArchivo = ((ConfiguracionSeñalAudio)(PanelConfiguracion_2.Children[0])).txtRutaArchivo.Text;
                        segundaSeñal = new SeñalAudio(rutaArchivo);
                        txtbox_tiempoInicial.Text = señal.TiempoInicial.ToString();
                        txtbox_tiempoFinal.Text = señal.TiempoFinal.ToString();
                        txtbox_muestreo.Text = señal.FrecuenciaMuestreo.ToString();
                        break;
                    default:
                        señal = null;
                        break;
                }
                if(CbTipoSeñal_2.SelectedIndex != 2 && segundaSeñal != null)
                {
                    segundaSeñal.TiempoInicial = tiempoInicial;
                    segundaSeñal.TiempoFinal = tiempoFinal;
                    segundaSeñal.FrecuenciaMuestreo = muestro;

                    segundaSeñal.construirSeñal();
                }
            }

            switch (CbOperacion.SelectedIndex)
            {
                case 0: //Escala de amplitud
                    double factorEscala =
                        double.Parse(
                            ((OperacionEscalaAmplitud)(PanelConfiguracionOperacion.Children[0])).txtFactorEscala.Text
                        );
                    señalResultante = Señal.escalaAmplitud(señal, factorEscala);
                    break;
                case 1: //Desplazamiento de amplitud
                    double factorDesplazamiento = double.Parse(
                        ((OperacionDesplazamientoAmplitud)(PanelConfiguracionOperacion.Children[0])).txtFactorDesplazamiento.Text
                        );
                    señalResultante = Señal.desplazamientoAmplitud(señal, factorDesplazamiento);
                    break;
                case 2: //Multiplicacion de señales
                    señalResultante = Señal.multiplicarSeñales(señal, segundaSeñal);
                    break;
                case 3: //Escala exponencial
                    double exponente = double.Parse(
                        ((OperacionEscalaExponencial)(PanelConfiguracionOperacion.Children[0])).txtExponente.Text);
                    señalResultante = Señal.escalaExponencial(señal, exponente);
                    break;
                case 4://Transformada de fourier
                    señalResultante = Señal.tranformadaFourier(señal);
                    break;
                default:
                    señalResultante = null;
                    break;
            }
            //Elige entre la 1ra y la resultante
            double amplitudMaxima = (señal.AmplitudMaxima >= señalResultante.AmplitudMaxima) ?
                señal.AmplitudMaxima : señalResultante.AmplitudMaxima;
            
            plnGrafica.Points.Clear();
            plnGraficaResultante.Points.Clear();
            plnGrafica_2.Points.Clear();

            if(segundaSeñal != null)
            {
                foreach (var muestra in segundaSeñal.Muestras)
                {
                    plnGrafica_2.Points.Add(adaptarCoordenadas(muestra.X, muestra.Y, tiempoInicial, tiempoFinal));
                }

                //Elige entre la mas grande de la 1ra y resultante y la segunda
                amplitudMaxima = (amplitudMaxima > segundaSeñal.AmplitudMaxima) ?
                    amplitudMaxima : segundaSeñal.AmplitudMaxima;
            }

            foreach (Muestra muestra in señal.Muestras)
            {
                plnGrafica.Points.Add(adaptarCoordenadas(muestra.X, muestra.Y, tiempoInicial, amplitudMaxima));
            }

            if(CbOperacion.SelectedIndex != -1)
            {
                foreach (Muestra muestra in señalResultante.Muestras)
                {
                    plnGraficaResultante.Points.Add(adaptarCoordenadas(muestra.X, muestra.Y, tiempoInicial, amplitudMaxima));
                }
            }

            //Hz
            if (CbOperacion.SelectedIndex == 4)
            {
                int indiceMaximo = 0;
                int indiceInicial =
                    (int)((690.0 * (double)(señalResultante.Muestras.Count)) / señalResultante.FrecuenciaMuestreo);
                int indiceFinal = 
                    (int)((950.0 * (double)(señalResultante.Muestras.Count)) / señalResultante.FrecuenciaMuestreo);

                for (int i=indiceInicial; i<indiceFinal; i++)
                {
                    if(señalResultante.Muestras[i].Y > señalResultante.Muestras[indiceMaximo].Y)
                    {
                        indiceMaximo = i;
                    }
                }
                double frecuencia = 
                    (double)(indiceMaximo) * señalResultante.FrecuenciaMuestreo / 
                    (double)(señalResultante.Muestras.Count);

                lblHzBaja.Text = frecuencia.ToString("N") + "Hz";

                //Hz min
                int indiceMaximaAlta = 0;
                int indiceInicialAlta =
                    (int)((1200.0*(double)señalResultante.Muestras.Count) /
                    (double)señalResultante.FrecuenciaMuestreo);
                int indiceFinalAlta =
                    (int)((1482.0 * (double)señalResultante.Muestras.Count) /
                    (double)señalResultante.FrecuenciaMuestreo);
                for(int i=indiceInicialAlta; i<=indiceFinalAlta; i++)
                {
                    if(señalResultante.Muestras[i].Y>señalResultante.Muestras[indiceMaximaAlta].Y)
                    {
                        indiceMaximaAlta = i;
                    }
                }
                double frecuenciaAlta =
                    (double)(indiceMaximaAlta * señalResultante.FrecuenciaMuestreo) /
                    (double)(señalResultante.Muestras.Count);

                lblHzAlta.Text = frecuenciaAlta.ToString("N") + "Hz";
            }



            //original
            lblLimiteSuperior.Text = amplitudMaxima.ToString("F");
            lblLimiteInferior.Text = "-" + amplitudMaxima.ToString("F");

            //resultado
            lblLimiteInferiorResultante.Text = "-" + amplitudMaxima.ToString("F");
            lblLimiteSuperiorResultante.Text = amplitudMaxima.ToString("F");

            //original
            plnEjeX.Points.Clear();
            plnEjeX.Points.Add(adaptarCoordenadas(tiempoInicial,0.0,tiempoInicial, amplitudMaxima));
            plnEjeX.Points.Add(adaptarCoordenadas(tiempoFinal, 0.0,tiempoInicial, amplitudMaxima));

            plnEjeY.Points.Clear();
            plnEjeY.Points.Add(adaptarCoordenadas(0.0, amplitudMaxima, tiempoInicial, amplitudMaxima));
            plnEjeY.Points.Add(adaptarCoordenadas(0.0, -amplitudMaxima, tiempoInicial, amplitudMaxima));

            //resultado
            plnEjeXResultante.Points.Clear();
            plnEjeXResultante.Points.Add(adaptarCoordenadas(tiempoInicial, 0.0, tiempoInicial, amplitudMaxima));
            plnEjeXResultante.Points.Add(adaptarCoordenadas(tiempoFinal, 0.0, tiempoInicial, amplitudMaxima));

            plnEjeYResultante.Points.Clear();
            plnEjeYResultante.Points.Add(adaptarCoordenadas(0.0, amplitudMaxima, tiempoInicial, amplitudMaxima));
            plnEjeYResultante.Points.Add(adaptarCoordenadas(0.0, -amplitudMaxima, tiempoInicial, amplitudMaxima));
        }

        public Point adaptarCoordenadas(double x, double y, double tiempoInicial, double amplitudMaxima)
        {
            return new Point((x - tiempoInicial) * scrGrafica.Width,
                (-1 * (
                y * (((scrGrafica.Height / 2.0) -25) / amplitudMaxima)) +
                (scrGrafica.Height / 2.0) ));
        }

        private void CbTipoSeñal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PanelConfiguracion.Children.Clear();
            switch(CbTipoSeñal.SelectedIndex)
            {
                case 0: //Exponencial
                    break;
                case 1: //Senoidal
                    PanelConfiguracion.Children.Add(new ConfiguracionSeñalSenoidal());
                    break;
                case 2: //Exponencial
                    PanelConfiguracion.Children.Add(new ConfiguracionSeñalExponencial());
                    break;
                case 3: //Audio
                    PanelConfiguracion.Children.Add(new ConfiguracionSeñalAudio());
                    break;
                default:
                    break;
            }
        }

        private void CbOperacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PanelConfiguracionOperacion.Children.Clear();
            mostrarSegundaSeñal(false);
            switch (CbOperacion.SelectedIndex)
            {
                case 0: //Escala de amplitud
                    PanelConfiguracionOperacion.Children.Add(new OperacionEscalaAmplitud());
                    break;
                case 1: //Desplazamiento de amplitud
                    PanelConfiguracionOperacion.Children.Add(new OperacionDesplazamientoAmplitud());
                    break;
                case 2: //Multiplicador de señales
                    mostrarSegundaSeñal(true);
                    break;
                case 3: //Escala Exponencial
                    PanelConfiguracionOperacion.Children.Add(new OperacionEscalaExponencial());
                    break;
                default:
                    break;
            }
        }

        private void CbTipoSeñal_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PanelConfiguracion_2.Children.Clear();
            switch (CbTipoSeñal_2.SelectedIndex)
            {
                case 0: //Exponencial
                    break;
                case 1: //Senoidal
                    PanelConfiguracion_2.Children.Add(new ConfiguracionSeñalSenoidal());
                    break;
                case 2: //Exponencial
                    PanelConfiguracion_2.Children.Add(new ConfiguracionSeñalExponencial());
                    break;
                case 3: //Audio
                    PanelConfiguracion_2.Children.Add(new ConfiguracionSeñalAudio());
                    break;
                default:
                    break;
            }
        }

        void mostrarSegundaSeñal(bool mostrar)
        {
            if (mostrar)
            {
                lblTipoSeñal_2.Visibility = Visibility.Visible;
                CbTipoSeñal_2.Visibility = Visibility.Visible;
                PanelConfiguracion_2.Visibility = Visibility.Visible;
            }
            else
            {
                lblTipoSeñal_2.Visibility = Visibility.Hidden;
                CbTipoSeñal_2.Visibility = Visibility.Hidden;
                PanelConfiguracion_2.Visibility = Visibility.Hidden;
            }
        }
    }
}
