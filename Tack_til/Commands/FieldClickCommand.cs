using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tack_til.Commands.Base;
using Tack_til.Models;
using Tack_til.ViewModels;

namespace Tack_til.Commands
{
    public class FieldClickCommand : Command
    {
        public override void Execute(object parameters)
        {
            object[] param = parameters as object[];
            ((GameViewModel) param[1]).Click((FieldModel)param[0]);

        }
    }
}
