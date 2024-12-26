using System.Collections.Generic;

namespace Brupper.Data.Entities;

public interface IMultiLingualEntity<TTranslation>
    where TTranslation : class, IEntityTranslation
{
    ICollection<TTranslation> Translations { get; set; }
}