using CommunityToolkit.Mvvm.Input;
using TheDiaryApp.Models;

namespace TheDiaryApp.PageModels;

public interface IProjectTaskPageModel
{
	IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
	bool IsBusy { get; }
}