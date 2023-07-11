using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using PlaywrightTest.Models;
using ReactiveUI;

namespace PlaywrightTest.ViewModels;

public class ErrorViewModel : ViewModelBase {
    private readonly ObservableCollection<ErrorItemViewModel> _errors = new();

    public ErrorViewModel() {
        Tree = new HierarchicalTreeDataGridSource<ErrorItemViewModel>(_errors) {
            Columns = {
                new HierarchicalExpanderColumn<ErrorItemViewModel>(
                    new TextColumn<ErrorItemViewModel, string>("", x => x.Name),
                    x => x.Children,
                    x => x.HasChildren,
                    x => x.IsExpanded),
                new TextColumn<ErrorItemViewModel, int>(
                    "Count",
                    x => x.Count)
            }
        };
    }

    public HierarchicalTreeDataGridSource<ErrorItemViewModel> Tree { get; }

    public ObservableCollection<ErrorItemViewModel> Errors => _errors;

    public class ErrorItemViewModel : ViewModelBase {
        private string _name;
        private int _count;
        private List<ErrorItemViewModel> _details = new();

        public string Name {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public int Count {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }


        public bool HasChildren => Children.Any();

        public List<ErrorItemViewModel> Children => _details;

        public bool IsExpanded { get; set; } = false;
    }

    public void Add(ReqRespTraceData trace) {
        var err = Errors.FirstOrDefault(e => e.Name == trace.StatusCode);
        if (err == null) {
            err = new ErrorItemViewModel() { Name = trace.StatusCode };
            Errors.Add(err);
        }

        err.Count += 1;
        err.Children.Add(new ErrorItemViewModel() { Name = trace.ResponseBody });
    }

    public void Clear() {
        Errors.Clear();
    }

    public void Add(Exception exc) {
        var err = Errors.FirstOrDefault(e => e.Name == exc.GetType().Name);
        if (err == null) {
            err = new ErrorItemViewModel() { Name = exc.GetType().Name };
            Errors.Add(err);
        }

        err.Count += 1;
        err.Children.Add(new ErrorItemViewModel() { Name = exc.Message });
    }
}