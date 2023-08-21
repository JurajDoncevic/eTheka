using eTheka.Domain;    
using MediatR;
using System.Collections.Generic;

namespace eTheka.Messaging.Requests;
public class GetCurrentFiguresRequest: IRequest<IEnumerable<Figure>>
{
}
