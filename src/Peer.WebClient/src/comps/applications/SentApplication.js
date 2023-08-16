import React, { useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import SubmittedApplicationStatus from './SubmittedApplicationStatus';

const SentApplication = (props) => {

    const {
        application,
        projectTitle,
        positionName,
        onApplicationSelected,
        onApplicationDeselected
    } = props;

    const [isSelected, setIsSelected] = useState(false);

    function onSelectionChanged(e) {
        setIsSelected(e.target.checked);
    }

    useEffect(() => {
        isSelected ?
            onApplicationSelected(application.id) :
            onApplicationDeselected(application.id);
    }, [isSelected])


    return <tr key={application.id} className={`hover:bg-blue-200 cursor-pointer border-b ${isSelected ? "bg-cyan-300" : ""}`}>
        <td className='py-4 px-2 pl-4 truncate underline text-blue-500 hover:text-blue-700'><Link to={application.projectUrl}>{projectTitle}</Link></td>
        <td className='py-4 px-2 truncate'>{positionName}</td>
        <td className='py-4 px-2 truncate'>{application.dateSubmitted}</td>
        <td className='py-4 px-2 truncate'>
            <SubmittedApplicationStatus></SubmittedApplicationStatus>
        </td>
        <td className='py-4 pr-4 px-2 text-center'>
            <form onChange={() => { }}>
                <input
                    type='checkbox'
                    checked={isSelected}
                    onChange={onSelectionChanged}></input>
            </form>
        </td>
    </tr>
}

export default SentApplication