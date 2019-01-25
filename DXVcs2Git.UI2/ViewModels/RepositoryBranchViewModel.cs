﻿using System;
using DXVcs2Git.UI2.Core;
using NGitLab.Models;
using ReactiveUI;

namespace DXVcs2Git.UI2.ViewModels {
    public class RepositoryBranchViewModel : ReactiveObject, IDisposable {
        BranchModelState state;
        string name;

        public IRepositoryModel Repository { get; }
        public IBranchModel Branch { get; }
        public BranchModelState State {
            get => this.state;
            private set => this.RaiseAndSetIfChanged(ref this.state, value);
        }
        public string Name {
            get => this.name;
            private set => this.RaiseAndSetIfChanged(ref this.name, value);
        }

        public RepositoryBranchViewModel(IRepositoryModel repository, IBranchModel branch) {
            Repository = repository;
            Branch = branch;
            State = BranchModelState.Initialized;
            Name = branch.Name;
        }
        
        public void Dispose() {
        }
    }
}