using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.ServiceModel
{
    public interface IServiceChannelFactory<out TServiceContract>
    {
        TServiceContract CreateChannel();
    }
}
