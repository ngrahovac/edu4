import React from 'react'

const Collaborator = (props) => {
    const {
        avatar,
        name,
        position,
        onVisited
    } = props;

    return (
        <div className='flex flex-row shrink-0 space-x-4'>
            <img src={avatar} width={80} height={80} className='rounded-full'></img>
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