import SubsectionTitle from '../../layout/SubsectionTitle';
import PositionCard from './PositionCard';
import ProjectDescriptor from './ProjectDescriptor';
import RecommendedFlair from './RecommendedFlair';
import { Link } from 'react-router-dom';
import ProjectTitle from './ProjectTitle';

const ProjectCard = (props) => {

    const {
        project,
    } = props;

    const maxPositionsShown = 2;

    let timeDifference = Date.now() - new Date(project.datePosted);
    let daysSince = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

    let notShownPositionsCount = 0;

    if (project.recommended) {
        let recommendedPositionsCount = project.positions.filter(p => p.recommended).length;
        if (recommendedPositionsCount > maxPositionsShown) {
            notShownPositionsCount = recommendedPositionsCount - maxPositionsShown;
        }
    } else {
        let positionsCount = project.positions.length;
        if (positionsCount > maxPositionsShown) {
            notShownPositionsCount = positionsCount - maxPositionsShown;
        }
    }

    return (
        <div className='border-4 border-pink-500 cursor-pointer'>
            <div className="flex flex-col gap-y-6 px-20 py-8">
                <div>
                    <div className='flex justify-between items-center flex-wrap gap-y-2'>
                        <div className='flex gap-x-4'>
                            <ProjectDescriptor
                                link={true}
                                value={project.author.fullName}
                                icon={
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="gray" className="w-12 h-12">
                                        <path strokeLinecap="round" strokeLinejoin="round" d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
                                    </svg>
                                }>
                            </ProjectDescriptor>

                            <ProjectDescriptor
                                value={<p>{daysSince} days ago</p>}>
                            </ProjectDescriptor>
                        </div>

                        {
                            project.recommended &&
                            <div className='flex flex-end'>
                                <RecommendedFlair></RecommendedFlair>
                            </div>
                        }

                    </div>
                </div>

                <div className='flex flex-col gap-y-2'>
                    <ProjectTitle>{project.title}</ProjectTitle>

                    <div className='flex gap-x-4 justify-between flex-wrap gap-y-2'>
                        <ProjectDescriptor
                            icon={
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="lightgray" className="w-5 h-5">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
                                </svg>
                            }
                            value={<p>starts <span className='font-black'>{project.duration.startDate}</span></p>}>
                        </ProjectDescriptor>

                        <ProjectDescriptor
                            icon={
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="lightgray" className="w-5 h-5">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
                                </svg>
                            }
                            value={<p><span className='font-black'>{project.positions.length}</span> open positions</p>}>
                        </ProjectDescriptor>

                        <ProjectDescriptor
                            icon={
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="lightgray" className="w-5 h-5">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
                                </svg>
                            }
                            value={<p><span className='font-black'>{project.collaborations.length}</span> collaborators</p>}>
                        </ProjectDescriptor>
                    </div>
                </div>

                <div className='flex flex-col gap-y-4'>
                    <SubsectionTitle title={`${project.recommended ? "Recommended positions" : "Positions"}`}></SubsectionTitle>

                    <div className='flex flex-col space-y-2'>
                        {
                            project.recommended &&

                            project.positions.filter(p => p.recommended).slice(0, maxPositionsShown).map((p, index) => <div key={index}>
                                <PositionCard position={p}></PositionCard>
                            </div>)
                        }

                        {
                            !project.recommended &&

                            project.positions.slice(0, maxPositionsShown).map((p, index) => <div key={index}>
                                <PositionCard position={p}></PositionCard>
                            </div>)
                        }
                    </div>
                </div>

                <div className='flex items-start'>
                    {
                        notShownPositionsCount > 0 &&
                        <ProjectDescriptor
                            value={<p>... and <span className='font-black'>{notShownPositionsCount}</span> more</p>}>
                        </ProjectDescriptor>
                    }
                </div>

                <div className='flex flex-row-reverse'>
                    <Link to={project.projectUrl}>
                        <p className='uppercase tracking-wide font-semibold text-indigo-500'>Learn more</p>
                    </Link>
                </div>
            </div>
        </div>
    )
}

export default ProjectCard