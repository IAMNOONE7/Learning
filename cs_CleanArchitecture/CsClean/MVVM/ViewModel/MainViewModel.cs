using CsClean.Application.UseCases;
using CsClean.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CsClean.Presentation.Wpf.MVVM.ViewModel
{
    // NOTE TO SELF: Thin VM — coordinates UI with use cases; no Infra references.
    public sealed class MainViewModel
    {
        private readonly GetAllLocalMessages _getAll;
        private readonly SaveLocalMessage _save;
        private readonly DeleteLocalMessageByName _delete;

        public ObservableCollection<LocalMessages> Messages { get; } = new();

        // UI bindings
        public string? NewMessageName { get; set; }         // “name” textbox
        public string? NewItemText { get; set; }            // first item text
        public LocalMessages? SelectedMessage { get; set; }  // selected item in list


        // Simple command to demo interaction
        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand RemoveSelectedCommand { get; }


        public MainViewModel(GetAllLocalMessages getAll, SaveLocalMessage save, DeleteLocalMessageByName delete)
        {
            _getAll = getAll; // DIP: depend on use case, not repo directly.
            _save = save;
            _delete = delete;
            LoadCommand = new RelayCommand(async _ => await LoadAsync());
            AddCommand = new RelayCommand(async _ => await AddAsync(), _ => CanAdd());
            RemoveSelectedCommand = new RelayCommand(async _ => await RemoveSelectedAsync(), _ => SelectedMessage != null);
            // (Optional) auto-load on startup for demo
            _ = LoadAsync();
        }


        private async Task LoadAsync(CancellationToken ct = default)
        {
            Messages.Clear();
            var data = await _getAll.ExecuteAsync(ct);
            foreach (var m in data) Messages.Add(m);
        }

        private bool CanAdd()
      => !string.IsNullOrWhiteSpace(NewMessageName) && !string.IsNullOrWhiteSpace(NewItemText);

        private async Task AddAsync()
        {
            // Real life: user fills a small form; we save to DB/file
            var msg = new LocalMessages(NewMessageName!.Trim(),
                       new[] { new LocalMessageItem(0, NewItemText!.Trim()) });

            await _save.ExecuteAsync(msg);
            // refresh list
            await LoadAsync();

            // clear inputs (quality-of-life)
            NewMessageName = string.Empty;
            NewItemText = string.Empty;
            // notify CanExecute changed
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        private async Task RemoveSelectedAsync()
        {
            if (SelectedMessage is null) return;
            await _delete.ExecuteAsync(SelectedMessage.Name);
            await LoadAsync();
        }
    }
}
