import React from 'react'

const Collaborator = (props) => {
    const {
        avatar,
        name,
        position,
        onVisited
    } = props;

    const blankAvatarUrl = "https://qcb.al/wp-content/uploads/2018/04/Blank_avatar-450x450.jpeg";

    return (
        <div className='flex flex-row shrink-0 space-x-4'>
            <img src={`${avatar == undefined ? blankAvatarUrl : avatar}`} width={80} height={80} className='rounded-xl'></img>

            <div className='flex flex-col'>
                <p
                    onClick={onVisited}
                    className='font-semibold text-lg'>
                    {name}
                </p>
                <p>{position}</p>
            </div>
        </div>
    )
}

export default Collaborator