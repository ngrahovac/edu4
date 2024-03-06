import React, { Fragment, useEffect } from 'react'
import { useState } from 'react';

const ProjectFilter = (props) => {
    const {
        projects,
        onProjectSelected,
        selectedProjectId = undefined
    } = props;

    const [selectedProject, setSelectedProject] = useState(selectedProjectId ?
        projects.find(p => p.id === selectedProjectId) : undefined);

    const handleSelectedProject = (e) => {
        const selectedProjectId = e.target.value;

        selectedProjectId == undefined ?
            setSelectedProject(undefined) :
            setSelectedProject(projects.find(p => p.id == selectedProjectId));
    };

    useEffect(() => {
        onProjectSelected(selectedProject);
    }, [selectedProject])


    return (
            <select
                value={selectedProject ? selectedProject.id : undefined}
                onChange={handleSelectedProject}
                className='rounded-full border-gray-200 text-gray-700 text-base'>
                <option
                    value={undefined}
                    className='rounded-xl'>
                    All projects
                </option>
                {projects.map(p => (
                    <Fragment key={p.id}>
                        <option value={p.id}>
                            {p.title}
                        </option>
                    </Fragment>
                ))}
            </select>
    )
}

export default ProjectFilter