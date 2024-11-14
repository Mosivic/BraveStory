using System.Collections.Generic;

namespace Miros.Core;

public interface IPersona
{


    T AttrSet<T>() where T : AttributeSet;
}