// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();
serviceCollection.AddGitHubClient();
