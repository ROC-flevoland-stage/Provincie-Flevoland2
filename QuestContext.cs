using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class QuestContext
{
    public QuestEventBus EventBus { get; }
    public QuestContext(QuestEventBus bus) => EventBus = bus;


}
