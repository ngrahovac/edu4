import React from 'react'
import Collaborator from './Collaborator'

const Author = (props) => {
    const {
        avatar,
        name,
        onVisited
    } = props;

    return (
        <Collaborator
            avatar={avatar}
            name={name}
            position="Author"
            onVisited={onVisited}>
        </Collaborator>
    )
}

export default Author