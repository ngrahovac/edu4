import React from 'react'

const MyCollaborations = (props) => {
    const {
        collaborations,
        projects
    } = props;

    return (
        <>
            collaborations count:
            <div>{collaborations.length}</div>
            projects count:
            <div>{projects.length}</div>
        </>
    )
}

export default MyCollaborations