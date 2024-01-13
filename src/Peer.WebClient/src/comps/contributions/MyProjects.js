import React from 'react'
import { Fragment } from 'react';
import { Link } from 'react-router-dom';

const MyProjects = (props) => {
    const {
        projects
    } = props;

    return (
        <div className='overflow-x-auto'>
            <table className='text-left w-full'>
                <thead>
                    <tr className=''>
                        <th className='py-4 px-2 pl-4 w-1/2 truncate'>Project</th>
                        <th className='py-4 px-2 truncate'>Date posted</th>
                    </tr>

                </thead>
                <tbody>
                    {
                        projects.map(p => <Fragment key={p.id}>
                            <tr className={`hover:bg-blue-200 cursor-pointer border-b`} >
                                <Link to={`/projects/${p.id}`}><td className='px-2 pl-4 py-4 underline'>{p.title}</td></Link>
                                <td className='px-2 py-4'>{p.datePosted}</td>
                            </tr>
                        </Fragment>)
                    }
                </tbody>
            </table>
        </div>
    )
}

export default MyProjects