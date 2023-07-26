using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Result<Photo>>
        {
            public IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Photo>>
        {
            //To save the photo in the database
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
                _photoAccessor = photoAccessor;
            }

            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                //From infrastructure where IPhotoAccessor insterface is implemented and in there is added claudinary
                var photoUploadResult = await _photoAccessor.AddPhoto(request.File);
                //result is User object with the photos collection
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                var photo = new Photo
                {
                    Url = photoUploadResult.Url,
                    Id = photoUploadResult.PublicId
                };

                //if user has not (any) photo then this is is going to be first and it's the main
                if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;
                //Add to the user collection Domain\AppUser.cs the  new created photo
                user.Photos.Add(photo);
                //save in database
                var result = await _context.SaveChangesAsync() > 0;
                //Application\Core\Result.cs
                if (result) return Result<Photo>.Success(photo);
                //Application\Core\Result.cs
                return Result<Photo>.Failure("Problem adding photo");
            }
        }
    }
}