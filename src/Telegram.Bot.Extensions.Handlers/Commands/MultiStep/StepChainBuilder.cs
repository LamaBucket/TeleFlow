using System.Runtime.InteropServices;
using LisBot.Common.Telegram.Factories.CommandFactories;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Commands.MultiStep;

public class StepChainBuilder
{
    public event Func<Task>? ChainFinished;

    public Stack<IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>> StepFactoryChain => new(_stepFactoryChain);

    public StepCommand? Head => _head;


    private readonly Stack<IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>> _stepFactoryChain;
    
    private StepCommand? _head;


    public void AddItemToChain(IHandlerFactoryWithArgs<StepCommand, Update, StepCommand> factory)
    {
        _stepFactoryChain.Push(factory);
    }

    public StepCommand RebuildWithPreviousStep()
    {
        var currentDepth = FindCurrentCommandDepth();

        var stepFactoryChain = CreateStepFactoryChainForNSteps(currentDepth + 1);

        return BuildChain(stepFactoryChain, true);
    }


    private int FindCurrentCommandDepth()
    {
        int depth = 0;
        StepCommand? current = _head;

        while (current != null)
        {
            depth++;
            current = current.Next;
        }

        return depth;
    }

    private Stack<IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>> CreateStepFactoryChainForNSteps(int n)
    {
        Stack<IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>> stack = new();

        var stepFactoryChain = StepFactoryChain.ToArray();

        n = Math.Min(n, _stepFactoryChain.Count);

        var toSkip = _stepFactoryChain.Count - n;

        Range range = new(toSkip, _stepFactoryChain.Count);

        var selectedFactories = stepFactoryChain.Take(range).Reverse();

        foreach (var factory in selectedFactories)
            stack.Push(factory);

        return stack;
    }

    public StepCommand BuildChain(IHandlerFactoryWithArgs<StepCommand, Update, StepCommand> factory, bool overwrite = false)
    {
        if (!_stepFactoryChain.Contains(factory))
            throw new ArgumentException("That Step Factory does not belong to that step factory chain", nameof(factory));

        var stack = new Stack<IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>();
        stack.Push(factory);

        return BuildChain(stack, overwrite);
    }

    public StepCommand BuildChain(bool overwrite = false)
    {
        return BuildChain(StepFactoryChain, overwrite);
    }

    private StepCommand BuildChain(Stack<IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>> stepFactoryChain, bool overwrite = false)
    {
        if(!overwrite && _head is not null)
            throw new InvalidOperationException("Next command already exists");

        if(stepFactoryChain.Count == 0)
            throw new InvalidOperationException("The Factory chain was empty");


        var last = stepFactoryChain.Pop().Create();

        StepCommand head = last;

        while(stepFactoryChain.Count != 0)
        {
            var factory = stepFactoryChain.Pop();

            factory.SetContext(head);
            head = factory.Create();
        }


        _head = head;
        _head.CommandFinished += OnNextCommandFinished;

        return last;
    }

    private async Task OnNextCommandFinished()
    {
        if(_head is null)
            throw new NullReferenceException(nameof(_head));

        _head.CommandFinished -= OnNextCommandFinished;
        _head = null;

        if(ChainFinished is not null)
            await ChainFinished.Invoke();
    }


    public StepChainBuilder()
    {
        _stepFactoryChain = new();
    }
}