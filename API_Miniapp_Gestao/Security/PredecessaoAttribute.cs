using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Redis;

/// <summary>
/// Indica que o m�todo da controller possui regras de predecess�o para execu��o.
/// Permite definir quais etapas anteriores (Anteriores) s�o necess�rias antes de acessar a etapa atual (AposChamada).
/// Usado para controlar o fluxo de chamadas e valida��o de etapas via middleware.
/// </summary>
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method)]
public class PredecessaoAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Lista de etapas anteriores que devem ser conclu�das antes de acessar o m�todo anotado.
    /// </summary>
    public int[] Anteriores { get; set; } = [];

    /// <summary>
    /// Identificador da etapa atual ap�s a chamada do m�todo.
    /// </summary>
    public int AposChamada { get; set; }

    /// <summary>
    /// Construtor que recebe etapas anteriores e a etapa atual.
    /// </summary>
    /// <param name="Anteriores">Etapas predecessoras necess�rias.</param>
    /// <param name="AposChamada">Etapa atual ap�s chamada.</param>
    public PredecessaoAttribute(int[] Anteriores, int AposChamada)
    {
        this.Anteriores = Anteriores;
        this.AposChamada = AposChamada;
    }

    /// <summary>
    /// Construtor que recebe apenas a etapa atual.
    /// </summary>
    /// <param name="AposChamada">Etapa atual ap�s chamada.</param>
    public PredecessaoAttribute(int AposChamada)
    {
        this.AposChamada = AposChamada;
    }
}
