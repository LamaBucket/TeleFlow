using LisBot.Common.Telegram.Factories.CommandFactories;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Commands.MultiStep;

public class StepChainBuilder
{
    public event Func<Task>? ChainFinished;

    public Stack<ICommandFactory<StepCommand, Update, StepCommand>> StepFactoryChain => new(_stepFactoryChain);

    public StepCommand? Head => _head;


    private readonly Stack<ICommandFactory<StepCommand, Update, StepCommand>> _stepFactoryChain;
    
    private StepCommand? _head;


    public void AddItemToChain(ICommandFactory<StepCommand, Update, StepCommand> factory)
    {
        _stepFactoryChain.Push(factory);
    }


    public StepCommand BuildChain(ICommandFactory<StepCommand, Update, StepCommand> factory, bool overwrite = false)
    {
        if(!_stepFactoryChain.Contains(factory))
            throw new Exception("That Step Factory does not belong to that step factory chain");

        var stack = new Stack<ICommandFactory<StepCommand, Update, StepCommand>>();
        stack.Push(factory);

        return BuildChain(stack, overwrite);
    }

    public StepCommand BuildChain(bool overwrite = false)
    {
        return BuildChain(StepFactoryChain, overwrite);
    }

    private StepCommand BuildChain(Stack<ICommandFactory<StepCommand, Update, StepCommand>> stepFactoryChain, bool overwrite = false)
    {
        if(!overwrite && _head is not null)
            throw new Exception("Next command already exists");

        if(stepFactoryChain.Count == 0)
            throw new Exception("The Factory chain was empty");


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
            throw new Exception("Something went wrong");

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