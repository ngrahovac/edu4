import React, { Fragment, useEffect } from 'react'
import { useState } from 'react';

const ProjectFilter = (props) => {
    const {
        projects,
        onProjectSelected,
        onProjectDeselected,
    } = props;

    const [selectedProject, setSelectedProject] = useState(undefined);

    const handleSelectedProject = (e) => {
        const selectedProjectId = e.target.value;

        selectedProjectId == undefined ?
            setSelectedProject(undefined) :
            setSelectedProject(projects.find(p => p.id == selectedProjectId));
    };

    useEffect(() => {
        if (selectedProject == undefined) {
            onProjectDeselected();
        } else {
            onProjectSelected(selectedProject);
        }
    }, [selectedProject])


    return (
        <div className='flex flex-col space-y-2'>
            <p>Filter by project:</p>
            <select
                value={selectedProject ? selectedProject.id : undefined}
                onChange={handleSelectedProject}
                className='rounded-xl w-64'>
                <option
                    value={undefined}
                    className='rounded-xl'>
                    All
                </option>
                {projects.map(p => (
                    <Fragment key={p.id}>
                        <option value={p.id}>
                            {p.title}
                        </option>
                    </Fragment>
                ))}
            </select>
        </div>
    )
}

export default ProjectFilter