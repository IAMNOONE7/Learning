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
using System.Windows.Shapes;

namespace cs_Asynchronous.Task_Async_Await_Basic
{
    /// <summary>
    /// Interaction logic for Task_Async_Await_Basic_View.xaml
    /// </summary>
    public partial class Task_Async_Await_Basic_View : Window
    {
        public Task_Async_Await_Basic_View()
        {
            InitializeComponent();
        }
        // Event handlers in WPF can be 'async void' (this is the one valid place).
        private async void Btn_Click(object sender, RoutedEventArgs e)
        {
            // UI update happens immediately
            Btn.IsEnabled = false;
            Status.Text = "Working... (UI stays responsive)";

            // This is the whole point: 'await' asynchronously waits without blocking the UI thread.
            // Replace Task.Delay with real async I/O later (HttpClient, file I/O, DB, etc.).
            await Task.Delay(2000);

            // Code after 'await' resumes on the UI thread by default in WPF.
            Status.Text = "Done.";
            Btn.IsEnabled = true;
        }

        private async void Btn1_Click(object sender, RoutedEventArgs e)
        {
            Btn1.IsEnabled = false;
            Status1.Text = "Calculating...";

            // Simulate work that returns a value
            int result = await ComputeAsync();  // Task<int>

            Status1.Text = $"Done. Result = {result}";
            Btn1.IsEnabled = true;
        }

        // Returns a Task<int>; 'await' uses the result later.
        private async Task<int> ComputeAsync()
        {
            await Task.Delay(1000); // simulate async operation
            return 42;              // value flows back to caller after 'await'
        }

        private async void Btn2_Click(object sender, RoutedEventArgs e)
        {
            Btn2.IsEnabled = false;
            Status2.Text = "Starting...";

            // First async call
            Status2.Text = "Step 1: preparing data...";
            await Task.Delay(1000); // simulate async work (1s)

            // Second async call
            Status2.Text = "Step 2: downloading...";
            await Task.Delay(2000); // simulate another async operation (2s)

            // Third async call
            Status2.Text = "Step 3: saving to file...";
            await Task.Delay(1500); // another await

            // Continue once all awaits finished
            Status2.Text = "All done!";
            Btn2.IsEnabled = true;
        }
    }
}
