using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.ApplicationModel;

namespace GestorTareasMovil
{
    // 5. Gestión del Ciclo de Vida (App Lifecycle)
    public partial class App : Application
    {
        public App()
        {
            MainPage = new NavigationPage(new TareasPage());
        }

        protected override void OnSleep()
        {
            // Guarda el estado general si el usuario sale de la app o recibe una llamada
            Preferences.Set("UltimoAcceso", DateTime.Now.ToString());
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }

    public class TareasPage : ContentPage
    {
        private Entry txtTarea;
        private Button btnAgregar;
        private Button btnCompartir;
        private StackLayout listaTareas;

        public TareasPage()
        {
            // 1. Interfaz Móvil Nativa (GUI)
            Title = "Gestor Académico UTP";
            Padding = new Thickness(20);

            txtTarea = new Entry { Placeholder = "Escribe una tarea universitaria..." };
            
            btnAgregar = new Button { Text = "Agregar Tarea", BackgroundColor = Colors.Green, TextColor = Colors.White };
            btnAgregar.Clicked += BtnAgregar_Clicked;

            btnCompartir = new Button { Text = "Compartir (Nativo)", BackgroundColor = Colors.Blue, TextColor = Colors.White };
            btnCompartir.Clicked += BtnCompartir_Clicked;

            listaTareas = new StackLayout { Spacing = 10, Margin = new Thickness(0, 20, 0, 0) };

            Content = new StackLayout
            {
                Children = { txtTarea, btnAgregar, btnCompartir, listaTareas }
            };
        }

        private async void BtnAgregar_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTarea.Text)) return;

            string nuevaTarea = txtTarea.Text;

            // 8. Optimización de Recursos (Asincronía en interfaz móvil)
            await Dispatcher.DispatchAsync(() => 
            {
                var lbl = new Label { Text = "• " + nuevaTarea, FontSize = 16 };

                // 6. Navegación Táctil y Gestual (Swipes)
                var swipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
                swipeGesture.Swiped += async (s, args) => 
                {
                    // 9. Invocación de Diálogos Móviles (DisplayAlert)
                    bool borrar = await DisplayAlert("Borrar", "¿Seguro que quieres borrar esta tarea?", "Sí", "No");
                    
                    // AHORA SÍ SE BORRA DE LA PANTALLA
                    if(borrar) 
                    {
                        listaTareas.Children.Remove(lbl);
                    }
                };

                lbl.GestureRecognizers.Add(swipeGesture);
                listaTareas.Children.Add(lbl);
            });

            // 4. Persistencia en Almacenamiento Sandboxed (Preferences) + 3. Offline
            int totalTareas = Preferences.Get("TotalTareas", 0) + 1;
            Preferences.Set("TotalTareas", totalTareas);
            Preferences.Set($"Tarea_{totalTareas}", nuevaTarea);

            // 2. Acceso a Hardware y Sensores (Haptic Feedback) - ARREGLADO
            try
            {
                // Toque háptico físico que no requiere permisos extra en Android
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
            catch (Exception)
            {
                // El catch(Exception) general evita que la app se cierre bajo cualquier error de permisos
            }

            txtTarea.Text = string.Empty;
        }

        private async void BtnCompartir_Clicked(object sender, EventArgs e)
        {
            // 7. Integración con Tareas del SO Móvil (Share API)
            int totalTareas = Preferences.Get("TotalTareas", 0);
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = $"Tengo {totalTareas} tareas pendientes en mi gestor académico.",
                Title = "Compartir Estatus Escolar"
            });
        }
    }
}